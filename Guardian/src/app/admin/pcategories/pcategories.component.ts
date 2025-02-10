import { Component } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { first } from 'rxjs'
import { userDto } from '../../dto/userDto';
import { AlertLevel } from '../../Enums/enums';
import { NavigationService } from '../../services/navigation.service';
import { UserStateService } from '../../services/user-state.service';
import { productCategoryDto } from '../../dto/productCategoryDto';
import { NgFor, NgIf } from '@angular/common';
import { ProductCategoryService } from '../../services/product-category.service';

@Component({
  selector: 'app-pcategories',
  imports: [ReactiveFormsModule, FormsModule, NgFor, NgIf],
  templateUrl: './pcategories.component.html',
  styleUrl: './pcategories.component.css'
})
export class PcategoriesComponent {
  cForm: FormGroup;
  cId!: number;
  userState: userDto | undefined;
  selectedItem: productCategoryDto | null = null;
  categoryList: productCategoryDto[] = [];

  constructor(
    private fb: FormBuilder,
    private categoryService: ProductCategoryService,
    private route: ActivatedRoute,
    private userService: UserStateService,
    public navigationService: NavigationService
  ) 
  {
    this.cForm = this.initializeForm();
  }

  ngOnInit(): void {
    this.userState = this.userService.getUserStateLocalStorage();
    this.cForm = this.initializeForm();
    this.navigationService.checkScreenSize();
    this.getAll();
  }

  // Inicializa el formulario
  initializeForm(): FormGroup {
    return this.fb.group({
      categoryId: [0],
      name: ['', Validators.required],
      description: [''],
      companyId: [this.userState?.companyId, Validators.required]
    });
  }

  loadData(Id: number): void {
    let companyId = this.userState?.companyId;
    this.categoryService.get(Id, companyId?? 0).pipe(first())
    .subscribe((c: productCategoryDto) => {
      this.cForm?.patchValue(c);
    });
  }

  newCustomer(): void {
    this.selectedItem = null;
    this.cForm?.reset();
    this.cForm?.patchValue({companyId: this.userState?.companyId, categoryId:0, name:'', description:''});
    window.scrollTo(0, 0);
  }

  save(): void {
    if (this.cForm?.invalid) {
      this.cForm.markAllAsTouched();
      return;
    }
    if (this.cForm?.valid) {
      
      this.categoryService.save(this.cForm.value).subscribe({
        next: (savedItem) => {
          this.getAll();
          this.selectItem(savedItem.categoryId);
          this.navigationService.showUIMessage("Categoria guardada (" + savedItem.categoryId + ")", AlertLevel.Sucess);
        },
        error: (e) => 
        {
          this.navigationService.showUIMessage(e.error.message);
        }
      });
    } else {
      console.log('Form is invalid');
    }
  }
  
  selectItem(id: number): void {
    let companyId = this.userState?.companyId;
    this.categoryService.get(id, companyId?? 0 ).pipe(first())
    .subscribe(c => {
      this.selectedItem = c;
      this.cForm?.patchValue(c);
      this.cForm.markAllAsTouched();
      this.navigationService.checkScreenSize();
      
    });
  }

  private getAll(): void {
    if(this.userState != null)
    {
      this.categoryService.getAll(this.userState.companyId).pipe(first())
      .subscribe(users => {
        this.categoryList = users.sort((a,b) => (a.name?? '').localeCompare((b.name?? '')));
      });
    }
    else
    {
      console.error("Estado de usuario invalido.");
    }
  }
}

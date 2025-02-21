import { Component } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { first } from 'rxjs'
import { userDto } from '../../dto/userDto';
import { AlertLevel } from '../../Enums/enums';
import { NavigationService } from '../../services/navigation.service';
import { UserStateService } from '../../services/user-state.service';
import { productCategoryDto } from '../../dto/productCategoryDto';
import { NgClass, NgFor, NgIf } from '@angular/common';
import { ProductCategoryService } from '../../services/product-category.service';

@Component({
  selector: 'app-pcategories',
  imports: [ReactiveFormsModule, FormsModule, NgFor, NgIf, NgClass],
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
    this.getAll('-1');
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

  newRecord(): void {
    this.selectedItem = null;
    this.cForm?.reset();
    this.cForm?.patchValue({companyId: this.userState?.companyId, categoryId:0, name:'', description:''});
    window.scrollTo(0, 0);
  }
  
  deleteRecord() {
    if (this.cForm?.valid) {
      let catInfo = this.cForm.value;
      this.categoryService.delete(catInfo).subscribe({
        complete: () => {
          this.getAll('-1');
          this.navigationService.showUIMessage("Categoria eliminada", AlertLevel.Info);
        },
        error: (err) => 
        {
          if (err.status === 500) 
          {
            this.navigationService.showUIMessage('No se pudo eliminar, verifica que la categoría no este en uso por algun producto.');  
          }
          else
            this.navigationService.showUIMessage(err.error.message);
        }
      });
    } 
    else 
    {
      this.navigationService.showUIMessage("El formulario contiene datos no válidos");
    }
  }

  save(): void {
    if (this.cForm?.invalid) {
      this.cForm.markAllAsTouched();
      return;
    }
    if (this.cForm?.valid) {
      let catInfo: productCategoryDto = this.cForm.value;
      this.categoryService.save(catInfo).subscribe({
        next: (savedItem) => {
          if(catInfo.categoryId == 0)
            this.categoryList.unshift(savedItem);
          
          this.selectItem(savedItem.categoryId);
          this.navigationService.showUIMessage("Categoria guardada (" + savedItem.categoryId + ")", AlertLevel.Sucess);
        },
        error: (e) => 
        {
          this.navigationService.showUIMessage(e.error);
        }
      });
    } 
    else 
    {
      this.navigationService.showUIMessage("El formulario contiene datos no válidos");
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

  filter()
  {
    let keyword = this.navigationService.searchItem;
    if (!keyword || keyword.trim() === "") {
      keyword="-1";
    }
    this.getAll(keyword);
  }

  private getAll(keyword:string): void {
    if(this.userState != null)
    {
      this.categoryService.getAll(this.userState.companyId, keyword).pipe(first())
      .subscribe(c => {
        this.categoryList = c.sort((a,b) => (a.name?? '').localeCompare((b.name?? '')));
        if(c.length > 0)
        {
          this.selectItem(c[0].categoryId);
        }
      });
    }
    else
    {
      console.error("Estado de usuario invalido.");
    }
  }
}

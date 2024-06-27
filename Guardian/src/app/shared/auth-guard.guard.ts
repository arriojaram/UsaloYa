import { inject } from "@angular/core"
import { AuthServiceService } from "../services/authorization.service";
import { Router } from "@angular/router";


export const AuthGuard = () => {
    const authService = inject(AuthServiceService);
    const router = inject(Router);

    let isloggedIn = authService.checkAuthentication();
    return !isloggedIn ? router.navigate(['/login']) : true;
}
import { inject } from "@angular/core"
import { AuthorizationService } from "../services/authorization.service";
import { Router } from "@angular/router";


export const AuthGuard = () => {
    const authService = inject(AuthorizationService);
    const router = inject(Router);

    let isloggedIn = authService.checkAuthentication();
    return !isloggedIn ? router.navigate(['/login']) : true;
}
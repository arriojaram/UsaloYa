import { inject } from "@angular/core"
import { AuthorizationService } from "../services/authorization.service";
import { Router, RouterStateSnapshot } from "@angular/router";


export const AuthGuard = (state: RouterStateSnapshot) => {

    const authService = inject(AuthorizationService);
    const router = inject(Router);

    const publicRoutes = ['/policy', '/agreements'];
    if (publicRoutes.includes(state.url)) {
      return true;
    }

    let isloggedIn = authService.checkAuthentication();
    return !isloggedIn ? router.navigate(['/login']) : true;
}
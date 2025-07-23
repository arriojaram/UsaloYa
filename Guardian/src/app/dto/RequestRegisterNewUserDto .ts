export interface RequestRegisterNewUserDto {
  userName: string;
  token: string;
  firstName: string;
  lastName?: string;
  phone: string;
  email: string;
  address?: string;
}

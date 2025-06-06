import { RequestRegisterNewUserDto } from "./RequestRegisterNewUserDto ";
import { companyDto } from "./companyDto";
import { GroupDto } from "./groupDto";

export interface RegisterUserAndCompanyDto {
  requestRegisterNewUserDto: RequestRegisterNewUserDto;
  companyDto: companyDto;
  groupDto: GroupDto;
}

import { RequestRegisterNewUserDto } from "./RequestRegisterNewUserDto ";
import { companyDto } from "./companyDto";
import { SaveQuestionDto } from "./SaveQuestionDto";


export interface RegisterUserQuestionnaireAndCompanyDto {
  requestRegisterNewUserDto: RequestRegisterNewUserDto;
  companyDto: companyDto;
 requestSaveQuestionnaireDto: SaveQuestionDto[];
  
  
}

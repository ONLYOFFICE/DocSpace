import { makeAutoObservable } from "mobx";

const regExps = {
  // source: https://regexr.com/3ok5o
  url: new RegExp(
    "([a-z]{1,2}tps?):\\/\\/((?:(?![/#?&]).)+)(\\/(?:(?:(?![#?&]).)+\\/)?)?((?:(?!\\.|$|\\?|#).)+)?(\\.(?:(?!\\?|$|#).)+)?(\\?(?:(?!$|#).)+)?(#.+)?"
  ),

  // source: https://regexr.com/2rhq7
  email: new RegExp(
    "[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?"
  ),

  // source: https://regexr.com/38pvb
  phone: new RegExp(
    "^\\s*(?:\\+?(\\d{1,3}))?([-. (]*(\\d{3})[-. )]*)?((\\d{3})[-. ]*(\\d{2,4})(?:[-.x ]*(\\d+))?)\\s*$"
  ),
};

class SsoFormStore {
  uploadXmlUrl = "";

  enableSso = true;

  spLoginLabel = "";

  // idpSettings
  entityId = "";
  ssoUrl = "";
  ssoBinding = "Redirect";
  sloUrl = "";
  sloBinding = "Redirect";
  nameIdFormat = "transient";

  newIdpCertificate = "";
  idpCertificates = [];

  // idpCertificateAdvanced
  idp_verifyAlgorithm = "rsa-sha1";
  idp_verifyAuthResponsesSign = true;
  idp_verifyLogoutRequestsSign = true;
  idp_verifyLogoutResponsesSign = true;
  idp_decryptAlgorithm = "aes128-cbc";
  ipd_decryptAssertions = false;

  spCertificates = [];

  // spCertificateAdvanced
  sp_decryptAlgorithm = null;
  sp_signingAlgorithm = "rsa-sha1";
  sp_signAuthRequests = true;
  sp_signLogoutRequests = true;
  sp_signLogoutResponses = true;
  sp_encryptAlgorithm = "aes128-cbc";
  sp_encryptAssertions = false;

  firstName = "";
  lastName = "";
  email = "";
  location = "";
  title = "";
  phone = "";

  hideAuthPage = false;

  // sp metadata
  sp_entityId = "";
  sp_assertionConsumerUrl = "";
  sp_singleLogoutUrl = "";

  // hide parts of form
  ServiceProviderSettings = true;
  ShowAdditionalParameters = true;
  SPMetadata = true;
  isModalVisible = false;

  // touched fields
  uploadXmlUrlTouched = false;
  spLoginLabelTouched = false;

  entityIdTouched = false;
  ssoUrlTouched = false;
  sloUrlTouched = false;

  firstNameTouched = false;
  lastNameTouched = false;
  emailTouched = false;
  locationTouched = false;
  titleTouched = false;
  phoneTouched = false;

  sp_entityIdTouched = false;
  sp_assertionConsumerUrlTouched = false;
  sp_singleLogoutUrlTouched = false;

  // errors
  uploadXmlUrlHasError = false;
  spLoginLabelHasError = false;

  entityIdHasError = false;
  ssoUrlHasError = false;
  sloUrlHasError = false;

  firstNameHasError = false;
  lastNameHasError = false;
  emailHasError = false;
  locationHasError = false;
  titleHasError = false;
  phoneHasError = false;

  sp_entityIdHasError = false;
  sp_assertionConsumerUrlHasError = false;
  sp_singleLogoutUrlHasError = false;

  // error messages
  uploadXmlUrlErrorMessage = null;
  spLoginLabelErrorMessage = null;

  entityIdErrorMessage = null;
  ssoUrlErrorMessage = null;
  sloUrlErrorMessage = null;

  firstNameErrorMessage = null;
  lastNameErrorMessage = null;
  emailErrorMessage = null;
  locationErrorMessage = null;
  titleErrorMessage = null;
  phoneErrorMessage = null;

  sp_entityIdErrorMessage = null;
  sp_assertionConsumerUrlErrorMessage = null;
  sp_singleLogoutUrlErrorMessage = null;

  constructor() {
    makeAutoObservable(this);
  }

  onSsoToggle = () => {
    this.enableSso = !this.enableSso;
  };

  onTextInputChange = (e) => {
    this[e.target.name] = e.target.value;
  };

  onBindingChange = (e) => {
    this.ssoBinding = e.target.value;
  };

  onComboBoxChange = (option, field) => {
    this[field] = option.key;
  };

  onHideClick = (e, label) => {
    this[label] = !this[label];
  };

  onCheckboxChange = (e) => {
    this[e.target.name] = e.target.checked;
  };

  onOpenModal = () => {
    this.isModalVisible = true;
  };

  onCloseModal = () => {
    this.isModalVisible = false;
  };

  onAddCertificate = () => {
    console.log("новый сертификат:", this.newIdpCertificate);
    this.isModalVisible = false;
    this.newIdpCertificate = "";
  };

  onBlur = (e) => {
    const field = e.target.name;
    const fieldTouched = `${field}Touched`;
    const fieldError = `${field}HasError`;
    const fieldErrorMessage = `${field}ErrorMessage`;

    const value = e.target.value;

    this[fieldTouched] = true;

    const validateResult = this.validate(value, this.getFieldType(field));

    if (typeof validateResult !== "string") {
      this[fieldError] = false;
      this[fieldErrorMessage] = null;
    } else {
      this[fieldError] = true;
      this[fieldErrorMessage] = validateResult;
    }
  };

  getFieldType = (field) => {
    if (field.toLowerCase().includes("url")) return "url";
    if (field.includes("entityId")) return "url";
    if (field.includes("email")) return "email";
    if (field.includes("phone")) return "phone";
    return "string";
  };

  validate = (string, type) => {
    string = string.trim();

    if (string.length === 0) return "EmptyFieldErrorMessage";

    if (type === "string") return true;

    return regExps[type].test(string) ? true : `${type}ErrorMessage`;
  };
}

const FormStore = new SsoFormStore();

export default FormStore;

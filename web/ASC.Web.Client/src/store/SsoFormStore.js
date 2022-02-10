import { makeAutoObservable } from "mobx";

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

  //hide parts of form
  ServiceProviderSettings = true;
  ShowAdditionalParameters = true;
  SPMetadata = true;
  isModalVisible = false;

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
  };
}

const FormStore = new SsoFormStore();

export default FormStore;

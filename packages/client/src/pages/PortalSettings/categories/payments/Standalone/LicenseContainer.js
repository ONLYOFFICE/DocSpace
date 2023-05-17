import React, { useEffect } from "react";
import { withRouter } from "react-router";
import { inject, observer } from "mobx-react";

import Text from "@docspace/components/text";
import FileInput from "@docspace/components/file-input";
import Button from "@docspace/components/button";
import { StyledButtonComponent } from "./StyledComponent";
import { toastr } from "@docspace/components";

const LicenseContainer = (props) => {
  const { t, setPaymentsLicense, acceptPaymentsLicense } = props;
  const onLicenseFileHandler = (file) => {
    let fd = new FormData();
    fd.append("files", file);

    setPaymentsLicense(null, fd);
  };

  const onClickUpload = () => {
    acceptPaymentsLicense(t);
  };
  return (
    <div className="payments_license">
      <Text fontWeight={700} fontSize="16px">
        {t("Payments:ActivateLicense")}
      </Text>

      <Text
        fontWeight={400}
        fontSize="14px"
        className="payments_license-description"
      >
        {t("ActivateUploadDescr")}
      </Text>
      <FileInput
        className="payments_file-input"
        scale
        size="base"
        accept=".lic"
        placeholder={t("Payments:UploadLicenseFile")}
        onInput={onLicenseFileHandler}
        // hasError={}
      />
      <StyledButtonComponent>
        <Button
          primary
          label={t("Common:Activate")}
          size="small"
          onClick={onClickUpload}
          // isLoading={}
          // isDisabled={}
        />
      </StyledButtonComponent>
    </div>
  );
};

export default inject(({ payments }) => {
  const { setPaymentsLicense, acceptPaymentsLicense } = payments;

  return { setPaymentsLicense, acceptPaymentsLicense };
})(withRouter(observer(LicenseContainer)));

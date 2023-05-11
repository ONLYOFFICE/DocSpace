import React, { useEffect } from "react";
import { withRouter } from "react-router";
import { inject, observer } from "mobx-react";

import Text from "@docspace/components/text";
import FileInput from "@docspace/components/file-input";
import Button from "@docspace/components/button";
import { StyledButtonComponent } from "./StyledComponent";

const LicenseContainer = (props) => {
  const { t } = props;
  const onLicenseFileHandler = (file) => {};

  const onClickUpload = () => {};
  return (
    <div className="payments_license">
      <Text fontWeight={700} fontSize="16px">
        {t("ActivateLicense")}
      </Text>

      <Text
        fontWeight={400}
        fontSize="14px"
        className="payments_license-description"
      >
        {t("UploadActivationLicense")}
      </Text>
      <FileInput
        className="payments_file-input"
        scale
        size="base"
        accept=".lic"
        placeholder={t("UploadLicenseFile")}
        onInput={onLicenseFileHandler}
        // hasError={}
      />
      <StyledButtonComponent>
        <Button
          primary
          label={t("Activate")}
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
  const {} = payments;

  return {};
})(withRouter(observer(LicenseContainer)));

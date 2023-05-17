import React, { useEffect } from "react";
import { withRouter } from "react-router";
import { inject, observer } from "mobx-react";

import Text from "@docspace/components/text";
import FileInput from "@docspace/components/file-input";
import Button from "@docspace/components/button";
import { StyledButtonComponent } from "./StyledComponent";

let timerId;
const LicenseContainer = (props) => {
  const {
    t,
    setPaymentsLicense,
    acceptPaymentsLicense,
    isLicenseCorrect,
    setIsLoading,
    isLoading,
  } = props;

  useEffect(() => {
    return () => {
      clearTimeout(timerId);
      timerId = null;
    };
  });
  const onLicenseFileHandler = (file) => {
    let fd = new FormData();
    fd.append("files", file);

    setPaymentsLicense(null, fd);
  };

  const onClickUpload = async () => {
    timerId = setTimeout(() => {
      setIsLoading(true);
    }, [200]);

    await acceptPaymentsLicense(t);

    setIsLoading(false);
    clearTimeout(timerId);
    timerId = null;
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
        isDisabled={isLoading}
      />
      <StyledButtonComponent>
        <Button
          primary
          label={t("Common:Activate")}
          size="small"
          onClick={onClickUpload}
          isLoading={isLoading}
          isDisabled={!isLicenseCorrect}
        />
      </StyledButtonComponent>
    </div>
  );
};

export default inject(({ payments }) => {
  const {
    setPaymentsLicense,
    acceptPaymentsLicense,
    isLicenseCorrect,
    setIsLoading,
    isLoading,
  } = payments;

  return {
    setPaymentsLicense,
    acceptPaymentsLicense,
    isLicenseCorrect,
    setIsLoading,
    isLoading,
  };
})(withRouter(observer(LicenseContainer)));

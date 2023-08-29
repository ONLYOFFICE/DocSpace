import React, { useEffect, useState } from "react";

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

  const [isLicenseUploading, setIsLicenseUploading] = useState(false);
  useEffect(() => {
    return () => {
      clearTimeout(timerId);
      timerId = null;
    };
  });
  const onLicenseFileHandler = async (file) => {
    timerId = setTimeout(() => {
      setIsLicenseUploading(true);
    }, [100]);

    let fd = new FormData();
    fd.append("files", file);

    await setPaymentsLicense(null, fd);

    clearTimeout(timerId);
    timerId = null;
    setIsLicenseUploading(false);
  };

  const onClickUpload = async () => {
    timerId = setTimeout(() => {
      setIsLoading(true);
    }, [200]);

    await acceptPaymentsLicense(t);

    clearTimeout(timerId);
    timerId = null;
    setIsLoading(false);
  };

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
        {t("ActivateUploadDescr")}
      </Text>
      <FileInput
        className="payments_file-input"
        scale
        size="base"
        accept=".lic"
        placeholder={t("UploadLicenseFile")}
        onInput={onLicenseFileHandler}
        isDisabled={isLicenseUploading || isLoading}
        isLoading={isLicenseUploading}
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
})(observer(LicenseContainer));

import React from "react";
import { withTranslation } from "react-i18next";
import ErrorContainer from "@docspace/common/components/ErrorContainer";

const RoomErrors = ({ t, tReady, isInvalid }) => {
  const headerText = isInvalid ? t("InvalidLink") : t("ExpiredLink");
  const bodyText = isInvalid ? t("LinkDoesNotExist") : t("LinkHasExpired");

  return tReady ? (
    <ErrorContainer headerText={headerText} bodyText={bodyText} />
  ) : (
    <></>
  );
};

export default withTranslation(["Errors"])(RoomErrors);

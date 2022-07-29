import React from "react";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import Box from "@appserver/components/box";
import Link from "@appserver/components/link";
import Text from "@appserver/components/text";

const HideButton = (props) => {
  const { t } = useTranslation("SingleSignOn");
  const { text, label, isAdditionalParameters, value, setHideLabel } = props;
  const marginProp = isAdditionalParameters ? null : "24px 0";
  const className = isAdditionalParameters
    ? "hide-additional-button"
    : "hide-button";

  const onClick = () => {
    setHideLabel(label);
  };

  return (
    <Box
      alignItems="center"
      displayProp="flex"
      flexDirection="row"
      marginProp={marginProp}
    >
      {!isAdditionalParameters && (
        <Text as="h2" fontSize="16px" fontWeight={600} noSelect>
          {text}
        </Text>
      )}

      <Link className={className} isHovered onClick={onClick} type="action">
        {value
          ? isAdditionalParameters
            ? t("HideAdditionalParameters")
            : t("Hide")
          : isAdditionalParameters
          ? t("ShowAdditionalParameters")
          : t("Show")}
      </Link>
    </Box>
  );
};

export default inject(({ ssoStore }) => {
  const { setHideLabel } = ssoStore;

  return {
    setHideLabel,
  };
})(observer(HideButton));

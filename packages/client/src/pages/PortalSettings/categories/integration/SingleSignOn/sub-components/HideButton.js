import React from "react";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import Box from "@docspace/components/box";
import Link from "@docspace/components/link";
import Text from "@docspace/components/text";

const HideButton = (props) => {
  const { t } = useTranslation("SingleSignOn");
  const {
    text,
    label,
    isAdditionalParameters,
    value,
    setHideLabel,
    isDisabled,
    id,
  } = props;
  const marginProp = isAdditionalParameters ? null : "24px 0";

  const onClick = () => {
    setHideLabel(label);
  };

  const onClickProp = isDisabled ? {} : { onClick: onClick };

  return (
    <Box
      alignItems="center"
      displayProp="flex"
      flexDirection="row"
      marginProp={marginProp}
    >
      {!isAdditionalParameters && (
        <Text
          as="h2"
          fontSize="16px"
          fontWeight={700}
          className="settings_unavailable"
          noSelect
        >
          {text}
        </Text>
      )}

      <Link
        id={id}
        className="hide-button settings_unavailable"
        isHovered
        {...onClickProp}
        type="action"
      >
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

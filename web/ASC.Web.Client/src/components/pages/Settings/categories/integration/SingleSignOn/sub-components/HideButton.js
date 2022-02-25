import React from "react";
import { observer } from "mobx-react";

import Box from "@appserver/components/box";
import FormStore from "@appserver/studio/src/store/SsoFormStore";
import Link from "@appserver/components/link";
import Text from "@appserver/components/text";

import { addArguments } from "../../../../utils/addArguments";

const HideButton = ({ label, t, isAdditionalParameters }) => {
  const hide = isAdditionalParameters ? "HideAdditionalParameters" : "Hide";
  const show = isAdditionalParameters ? "ShowAdditionalParameters" : "Show";
  const marginProp = isAdditionalParameters ? null : "24px 0";
  const className = isAdditionalParameters
    ? "hide-additional-button"
    : "hide-button";

  const onClick = addArguments(FormStore.onHideClick, label);

  return (
    <Box
      alignItems="center"
      displayProp="flex"
      flexDirection="row"
      marginProp={marginProp}
    >
      {!isAdditionalParameters && (
        <Text as="h2" fontSize="16px" fontWeight={600}>
          {t(label)}
        </Text>
      )}

      <Link className={className} isHovered onClick={onClick} type="action">
        {FormStore[label] ? t(hide) : t(show)}
      </Link>
    </Box>
  );
};

export default observer(HideButton);

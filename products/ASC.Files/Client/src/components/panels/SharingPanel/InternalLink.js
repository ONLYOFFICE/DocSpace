import React from "react";
import copy from "copy-to-clipboard";

import toastr from "@appserver/components/toast/toastr";
import Text from "@appserver/components/text";

import { StyledInternalLink } from "./StyledSharingPanel";

const InternalLink = ({ t, internalLink, style }) => {
  const onCopyInternalLinkAction = React.useCallback(() => {
    copy(internalLink);
    toastr.success(t("Translations:LinkCopySuccess"));
  }, [internalLink]);

  return (
    <StyledInternalLink style={style}>
      <Text trucate className={"internal-link__link-text"}>
        {t("InternalLink")}
      </Text>
      <Text
        className={"internal-link__copy-text"}
        onClick={onCopyInternalLinkAction}
      >
        {t("Translations:Copy")}
      </Text>
    </StyledInternalLink>
  );
};

export default React.memo(InternalLink);

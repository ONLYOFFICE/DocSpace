import React from "react";

import Text from "@appserver/components/text";

import { StyledInternalLink } from "./StyledSharingPanel";

const InternalLink = ({ linkText, copyText, onCopyInternalLink }) => {
  const onCopyInternalLinkAction = React.useCallback(() => {
    onCopyInternalLink && onCopyInternalLink();
  }, [onCopyInternalLink]);

  return (
    <StyledInternalLink>
      <Text className={"internal-link__link-text"}>{linkText}</Text>
      <Text
        className={"internal-link__copy-text"}
        onClick={onCopyInternalLinkAction}
      >
        {copyText}
      </Text>
    </StyledInternalLink>
  );
};

export default React.memo(InternalLink);

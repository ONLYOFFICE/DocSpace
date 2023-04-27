import Avatar from "@docspace/components/avatar";
import React from "react";
import Link from "@docspace/components/link";
import IconButton from "@docspace/components/icon-button";
import CopyReactSvgUrl from "PUBLIC_DIR/images/copy.react.svg?url";
import VerticalDotsReactSvgUrl from "PUBLIC_DIR/images/vertical-dots.react.svg?url";
import EyeReactSvgUrl from "PUBLIC_DIR/images/eye.react.svg?url";
import { StyledLinkRow } from "./StyledPublicRoom";

const LinkRow = (props) => {
  const { t, ...rest } = props;

  return (
    <StyledLinkRow {...rest}>
      <Avatar className="avatar" size="min" source={EyeReactSvgUrl} />

      <Link isHovered type="action" fontSize="14px" fontWeight={600}>
        {t("SharingPanel:ExternalLink")}
      </Link>

      <div className="external-row-icons">
        <IconButton
          className="copy-icon"
          size={16}
          iconName={CopyReactSvgUrl}
          // onClick={onClose}
        />

        <IconButton
          className="context-menu-icon"
          size={16}
          iconName={VerticalDotsReactSvgUrl}
          // onClick={onClose}
        />
      </div>
    </StyledLinkRow>
  );
};

export default LinkRow;

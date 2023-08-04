import React, { ForwardedRef, useMemo } from "react";

import Text from "@docspace/components/text";
import ContextMenu from "@docspace/components/context-menu";

import { StyledMobileDetails } from "../../StyledComponents";

import type MobileDetailsProps from "./MobileDetails.props";

import BackArrow from "PUBLIC_DIR/images/viewer.media.back.react.svg";
import MediaContextMenu from "PUBLIC_DIR/images/vertical-dots.react.svg";

function MobileDetails(
  {
    icon,
    title,
    isError,
    isPreviewFile,
    onHide,
    onMaskClick,
    onContextMenu,
    contextModel,
  }: MobileDetailsProps,
  ref: ForwardedRef<ContextMenu>
) {
  const contextMenuHeader = useMemo(
    () => ({
      icon: icon,
      title: title,
    }),
    [icon, title]
  );

  return (
    <StyledMobileDetails>
      <BackArrow className="mobile-close" onClick={onMaskClick} />
      <Text fontSize="14px" color="#fff" className="title">
        {title}
      </Text>
      {!isPreviewFile && !isError && (
        <div className="details-context">
          <MediaContextMenu
            className="mobile-context"
            onClick={onContextMenu}
          />
          <ContextMenu
            ref={ref}
            withBackdrop={true}
            onHide={onHide}
            header={contextMenuHeader}
            getContextModel={contextModel}
          />
        </div>
      )}
    </StyledMobileDetails>
  );
}

export default React.memo(
  React.forwardRef<ContextMenu, MobileDetailsProps>(MobileDetails)
);

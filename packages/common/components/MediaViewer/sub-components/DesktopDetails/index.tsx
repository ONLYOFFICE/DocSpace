import React from "react";

import Text from "@docspace/components/text";
import IconButton from "@docspace/components/icon-button";

import { ControlBtn } from "../../StyledComponents";

import ViewerMediaCloseSvgUrl from "PUBLIC_DIR/images/viewer.media.close.svg?url";

type DesktopDetailsProps = {
  title: string;
  onMaskClick: VoidFunction;
};

function DesktopDetails({ onMaskClick, title }: DesktopDetailsProps) {
  return (
    <div className="details">
      <Text
        isBold
        fontSize="14px"
        className="title"
        title={undefined}
        tag={undefined}
        as={undefined}
        fontWeight={undefined}
        color={undefined}
        textAlign={undefined}
      >
        {title}
      </Text>
      <ControlBtn onClick={onMaskClick} className="mediaPlayerClose">
        <IconButton
          color={"#fff"}
          iconName={ViewerMediaCloseSvgUrl}
          size={28}
          isClickable
        />
      </ControlBtn>
    </div>
  );
}

export default DesktopDetails;

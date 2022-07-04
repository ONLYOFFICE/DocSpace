import React from "react";
import { inject, observer } from "mobx-react";
import styled, { css } from "styled-components";

import { isMobile } from "react-device-detect";

import RowContent from "@appserver/components/row-content";
import Link from "@appserver/components/link";
import Text from "@appserver/components/text";
import { RoomsType } from "@appserver/common/constants";

const StyledRowContent = styled(RowContent)`
  .row-content_tablet-side-info {
    font-weight: 600;
    font-size: 12px;
    line-height: 16px;
    color: #a3a9ae;

    width: 100%;

    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;

    user-select: none;
  }
`;

const RoomsRowContent = ({ title, roomType, tags, sectionWidth }) => {
  const tagsText = tags?.length > 0 ? tags.join(" | ") : "";

  const getRoomType = () => {
    switch (roomType) {
      case RoomsType.FillingFormsRoom:
        return "Filling form";
      case RoomsType.EditingRoom:
        return "Editing";
      case RoomsType.CustomRoom:
        return "Custom";
      case RoomsType.ReadOnlyRoom:
        return "Viewing";
      case RoomsType.ReviewRoom:
        return "Review";
    }
  };

  return (
    <>
      <StyledRowContent
        sectionWidth={sectionWidth}
        isMobile={isMobile}
        isFile={false}
      >
        <Link
          className="row_content-link"
          containerWidth="55%"
          type="page"
          fontWeight="600"
          fontSize="15px"
          target="_blank"
          title={title}
          isTextOverflow={true}
        >
          {title}
        </Link>

        <div className="badges">
          <div></div>
          <div></div>{" "}
        </div>

        <Text
          className="row_type-text"
          containerMinWidth="200px"
          containerWidth="15%"
          fontSize="12px"
          fontWeight={400}
          truncate={true}
          noSelect={true}
        >
          {getRoomType()}
        </Text>

        <Text
          className="row_tags-text"
          containerMinWidth="200px"
          containerWidth="15%"
          fontSize="12px"
          fontWeight={400}
          truncate={true}
          noSelect={true}
        >
          {tagsText}
        </Text>
      </StyledRowContent>
    </>
  );
};

export default inject(() => {})(observer(RoomsRowContent));

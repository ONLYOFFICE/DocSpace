import React from "react";
import styled from "styled-components";

import Tags from "@docspace/components/Tags";

import { smallTablet } from "../../utils/device";
import Tag from "../../tag";
import { Base } from "../../themes";

const StyledPreviewTile = styled.div`
  background: ${(props) =>
    props.theme.createEditRoomDialog.previewTile.background};
  width: 214px;
  border: 1px solid
    ${(props) => props.theme.createEditRoomDialog.previewTile.borderColor};
  height: 120px;
  border-radius: 12px;

  @media ${smallTablet} {
    display: none;
  }

  .tile-header {
    display: flex;
    flex-direction: row;
    align-items: center;
    gap: 12px;
    padding: 15px;
    border-bottom: 1px solid
      ${(props) => props.theme.createEditRoomDialog.previewTile.borderColor};

    &-icon {
      width: 32px;
      height: 32px;
      border: 1px solid
        ${(props) =>
          props.theme.createEditRoomDialog.previewTile.iconBorderColor};
      border-radius: 6px;
      img {
        user-select: none;
        height: 32px;
        width: ${(props) => (props.isGeneratedPreview ? "32px" : "auto")};
        border-radius: 6px;
      }
    }
    &-title {
      font-weight: 600;
      font-size: 16px;
      line-height: 22px;
      white-space: nowrap;
      overflow: hidden;
      text-overflow: ellipsis;
      user-select: none;
    }
  }
  .tile-tags {
    box-sizing: border-box;
    max-width: 100%;
    display: flex;
    align-items: center;
    justify-content: start;
    padding: 15px;

    .type_tag {
      user-select: none;
      box-sizing: border-box;
      max-width: 100%;
      white-space: nowrap;
      overflow: hidden;
      text-overflow: ellipsis;
    }
  }
`;
StyledPreviewTile.defaultProps = { theme: Base };

const PreviewTile = ({ title, previewIcon, tags, defaultTagLabel }) => {
  return (
    <StyledPreviewTile>
      <div className="tile-header">
        <img className="tile-header-icon" src={previewIcon} alt={title} />
        <div className="tile-header-title">{title}</div>
      </div>
      <div className="tile-tags">
        {tags.length ? (
          <Tags columnCount={2} tags={tags} />
        ) : (
          <Tag
            className="type_tag"
            tag="script"
            label={defaultTagLabel}
            isDefault
          />
        )}
      </div>
    </StyledPreviewTile>
  );
};

export default PreviewTile;

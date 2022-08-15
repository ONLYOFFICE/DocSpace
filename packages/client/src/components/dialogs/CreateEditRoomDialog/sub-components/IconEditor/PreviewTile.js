import React from "react";
import styled from "styled-components";

import { smallTablet } from "@docspace/components/utils/device";

import Tags from "@docspace/common/components/Tags";

const StyledPreviewTile = styled.div`
  background: #ffffff;
  width: 214px;
  border: 1px solid #d0d5da;
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
    border-bottom: 1px solid #d0d5da;

    &-icon {
      width: 32px;
      height: 32px;
      border: 1px solid #eceef1;
      border-radius: 6px;
      img {
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
    }
  }
  .tile-tags {
    display: flex;
    align-items: center;
    justify-content: start;
    padding: 15px;
  }
`;

const PreviewTile = ({ title, previewIcon, tags }) => {
  return (
    <StyledPreviewTile>
      <div className="tile-header">
        <div className="tile-header-icon">
          <img src={previewIcon} alt={title} />
        </div>
        <div className="tile-header-title">{title}</div>
      </div>
      <div className="tile-tags">
        <Tags columnCount={3} tags={tags} />
      </div>
    </StyledPreviewTile>
  );
};

export default PreviewTile;

import styled from "styled-components";
import { isMobile, isTablet } from "react-device-detect";
import { Base } from "@docspace/components/themes";

const StyledThumbnail = styled.div`
  display: flex;
  justify-content: center;
  align-items: center;
  width: 100%;
  height: ${isMobile ? (isTablet ? "240" : "188") : "200"}px;
  img {
    border: ${(props) => `solid 1px ${props.theme.infoPanel.borderColor}`};
    border-radius: 6px;
    width: 100%;
    height: 100%;
    object-fit: cover;
    object-position: ${(props) => (props.isImageOrMedia ? "center" : "top")};
  }
`;

const StyledNoThumbnail = styled.div`
  height: auto;
  width: 100%;
  display: flex;
  justify-content: center;
  .no-thumbnail-img {
    height: 96px;
    width: 96px;
  }
  .is-room {
    border-radius: 16px;
  }
  .custom-logo {
    outline: 1px solid
      ${(props) => props.theme.infoPanel.details.customLogoBorderColor};
  }

  .room-title {
    font-size: 41px;
    font-weight: 700;
    line-height: 56px;
  }
`;

StyledThumbnail.defaultProps = { theme: Base };
StyledNoThumbnail.defaultProps = { theme: Base };

export { StyledThumbnail, StyledNoThumbnail };

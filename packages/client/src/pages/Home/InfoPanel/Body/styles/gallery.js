import styled from "styled-components";

import { Base } from "@docspace/components/themes";

const StyledGalleryThumbnail = styled.div`
  box-sizing: border-box;
  width: 100%;
  height: 346px;
  overflow: hidden;
  border: ${(props) =>
    `solid 1px ${props.theme.infoPanel.gallery.borderColor}`};
  border-radius: 6px;
  -webkit-tap-highlight-color: rgba(0, 0, 0, 0);

  .info-panel_gallery-img {
    display: block;
    margin: 0 auto;
  }
`;

StyledGalleryThumbnail.defaultProps = { theme: Base };

export { StyledGalleryThumbnail };

import styled, { css } from "styled-components";
import {
  mobile,
  tablet,
  smallTablet,
  desktop,
  size,
  hugeDesktop,
} from "../utils/device";
import NoUserSelect from "../utils/commonStyles";
import { isMobileOnly } from "react-device-detect";

const EmptyPageStyles = css`
  grid-row-gap: 9px;
  margin: 0 auto !important;

  .ec-image {
    height: 100px;
  }

  .ec-desc {
    max-width: 348px;
    line-height: 16px;
    margin-top: 0;
  }

  .ec-buttons {
    max-width: 285px;
  }

  .empty-folder_container-links {
    align-items: start;
    margin: 16px 0 !important;
  }

  @media ${smallTablet} {
    .ec-image {
      ${isMobileOnly &&
      css`
        height: 72px;
      `}
    }

    .ec-header {
      padding-top: 22px;
    }
  }

  @media ${desktop} {
    .ec-desc {
      max-width: 618px;
    }

    .ec-buttons {
      max-width: none;
    }
  }
`;

const MobileView = css`
  grid-template-areas:
    "img img img"
    "headerText headerText headerText"
    ${(props) =>
      props.subheadingText && `"subheadingText subheadingText subheadingText"`}
    ${(props) =>
      props.descriptionText &&
      `"descriptionText descriptionText descriptionText"`}
    "button button button";

  .ec-header {
    ${isMobileOnly &&
    css`
      padding: 0px;
    `}
  }
  .ec-header,
  .ec-subheading,
  .ec-desc,
  .ec-image,
  .ec-buttons {
    padding-left: 16px;
  }

  .ec-image {
    ${isMobileOnly &&
    css`
      height: 72px;
    `}

    margin-left: 0;
  }
`;

const EmptyContentBody = styled.div`
  margin: 0 auto;
  padding: 37px 0;
  grid-template-columns: 150px 1fr;

  display: grid;
  grid-template-areas:
    "img headerText"
    ${(props) => props.subheadingText && `"img subheadingText"`}
    ${(props) => props.descriptionText && `"img descriptionText"`}
    "img button";

  grid-column-gap: 16px;
  grid-row-gap: 10px;
  max-width: 800px;

  grid-template-rows: max-content;
  .ec-image {
    grid-area: img;
    margin: 16px 0 0 auto;
    ${NoUserSelect}
  }

  .ec-header {
    grid-area: headerText;
    font-size: 16px;
    padding-top: 16px;
    @media (max-width: 375px) {
      margin-top: 5px;
    }
  }

  .ec-subheading {
    grid-area: subheadingText;
  }

  .ec-desc {
    grid-area: descriptionText;
    line-height: 18px;
    margin-top: 2px;
  }

  .ec-buttons {
    grid-area: button;
  }

  @media ${hugeDesktop} {
    margin-left: 113px;
    margin-right: 113px;
  }

  @media ${tablet} {
    grid-template-columns: none;
    max-width: 480px;
    margin: 0 auto;
  }

  @media (orientation: portrait) {
    @media (max-width: 768px) {
      margin-left: 113px;
      margin-right: 113px;

      .ec-image {
        max-height: 100px;
      }
    }

    @media (max-width: 428px) {
      ${MobileView}
    }
  }

  ${(props) =>
    (props.isEmptyPage || props.isEmptyFolderContainer) && `${EmptyPageStyles}`}

  ${(props) =>
    props.sectionWidth <= size.smallTablet &&
    !isMobileOnly &&
    css`
      ${MobileView}

      .ec-desc {
        max-width: none;
      }

      .ec-header {
        padding-top: 22px;
      }

      max-width: none !important;
      margin: 0 auto !important;
    `}
`;

const EmptyContentImage = styled.img.attrs((props) => ({
  src: props.imageSrc,
  alt: props.imageAlt,
}))`
  background: no-repeat 0 0 transparent;
`;

export { EmptyContentBody, EmptyContentImage };

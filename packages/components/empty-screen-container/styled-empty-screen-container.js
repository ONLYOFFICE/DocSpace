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

  .ec-image {
    height: 100px;
  }

  .ec-desc {
    max-width: 348px;
    line-height: 16px;
    margin-top: 0;
  }

  .empty-folder_container-links {
    align-items: start;
    margin: 16px 0 !important;
  }

  max-width: 800px;

  @media ${hugeDesktop} {
    margin: 0 13%;
    width: 74%;
    min-width: 480px;
  }

  @media ${tablet} {
    max-width: 480px;
    margin: 0 auto;
  }

  @media (max-width: 768px) {
    ${(props) =>
      props.sectionWidth > size.smallTablet &&
      css`
        margin: 0 13% !important;
        width: 74%;
        min-width: auto;
      `}
  }

  @media ${smallTablet} {
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
    padding-top: 0px;
  }
  .ec-header,
  .ec-subheading,
  .ec-desc,
  .ec-image,
  .ec-buttons {
    padding-left: 16px;
  }

  .ec-image {
    height: 75px;
    margin-left: 0;
  }
`;

const EmptyContentBody = styled.div`
  margin: 0 auto;

  padding: ${(props) => (props.isEmptyFolderContainer ? "37px 0" : "64px 0")};
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

  @media ${tablet} {
    grid-template-columns: none;
    max-width: 480px;
  }

  @media (orientation: portrait) {
    @media (max-width: 768px) {
      padding-top: ${(props) => !props.isEmptyFolderContainer && "0px"};

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
    (props.isEmptyPage || props.isEmptyFolderContainer) &&
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

      .ec-image {
        height: 100px !important;
      }
    `}

    ${(props) =>
    (props.isEmptyPage || props.isEmptyFolderContainer) &&
    isMobileOnly &&
    css`
      .ec-image {
        margin-top: ${(props) => props.isEmptyFolderContainer && "0px"};
        height: ${(props) => props.isEmptyPage && "72px !important"};
      }
    `}


    ${(props) =>
    (props.isEmptyPage || props.isEmptyFolderContainer) &&
    props.sectionWidth <= size.smallTablet &&
    css`
      max-width: none !important;
      width: auto !important;
      min-width: auto !important;
      margin: 0 !important;
    `}
`;

const EmptyContentImage = styled.img.attrs((props) => ({
  src: props.imageSrc,
  alt: props.imageAlt,
}))`
  background: no-repeat 0 0 transparent;
`;

export { EmptyContentBody, EmptyContentImage };

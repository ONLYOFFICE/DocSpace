import styled, { css } from "styled-components";
import { mobile, tablet, smallTablet } from "../utils/device";
import NoUserSelect from "../utils/commonStyles";

const EmptyPageStyles = css`
  grid-row-gap: 9px;

  .ec-image {
    height: 100px;
  }

  .ec-desc {
    max-width: 618px;
  }

  .ec-header {
    font-size: 16px;
  }

  .ec-desc {
    line-height: 16px;
    margin-top: 0;
  }

  .empty-folder_container-links {
    align-items: center;
    margin: 16px 0 !important;
  }

  @media ${smallTablet} {
    .ec-image {
      height: 72px;
    }

    .ec-header {
      padding-top: 22px;
    }
  }
`;

const EmptyContentBody = styled.div`
  margin: 0 auto;
  padding: 64px 0;
  grid-template-columns: 150px 1fr;

  @media ${tablet} {
    grid-template-columns: none;
  }

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

  @media (orientation: portrait) {
    @media (max-width: 768px) {
      padding-top: 0px;
      max-width: 700px;

      .ec-image {
        max-height: 100px;
      }
    }

    @media (max-width: 428px) {
      grid-template-areas:
        "img img img"
        "headerText headerText headerText"
        ${(props) =>
          props.subheadingText &&
          `"subheadingText subheadingText subheadingText"`}
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
    }
  }

  ${(props) => props.isEmptyPage && `${EmptyPageStyles}`}
`;

const EmptyContentImage = styled.img.attrs((props) => ({
  src: props.imageSrc,
  alt: props.imageAlt,
}))`
  background: no-repeat 0 0 transparent;
`;

export { EmptyContentBody, EmptyContentImage };

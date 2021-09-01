import styled from "styled-components";
import { mobile } from "../utils/device";
import NoUserSelect from "../utils/commonStyles";

const EmptyContentBody = styled.div`
  margin: 0 auto;
  padding: 64px 0;

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
    margin: 0 0 0 auto;
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
    margin-top: -1px;
  }

  .ec-desc {
    grid-area: descriptionText;
    line-height: 18px;
    margin-top: 2px;
  }

  .ec-buttons {
    grid-area: button;
    margin-top: -1px;
  }

  @media (orientation: portrait) {
    @media (max-width: 768px) {
      padding-top: 0px;
      max-width: 700px;

      .ec-image {
        max-height: 100px;
      }
    }

    @media ${mobile} {
      min-width: 343px;
      grid-template-areas:
        "img"
        "headerText"
        ${(props) => props.subheadingText && `"subheadingText"`}
        ${(props) => props.descriptionText && `"descriptionText"`}
        "button";

      .ec-header {
        padding-top: 0px;
      }
      .ec-header,
      .ec-subheading,
      .ec-desc,
      .ec-buttons {
        padding-left: 16px;
      }

      .ec-image {
        height: 75px;
        margin-left: 0;
      }
    }
  }
`;

const EmptyContentImage = styled.img.attrs((props) => ({
  src: props.imageSrc,
  alt: props.imageAlt,
}))`
  background: no-repeat 0 0 transparent;
`;

export { EmptyContentBody, EmptyContentImage };

import styled, { css } from "styled-components";
import { Base } from "../../themes";
import { tablet, desktop, mobile } from "../../utils/device";

const StyledAvatarEditorBody = styled.div`
  max-width: ${(props) => props.theme.avatarEditorBody.maxWidth};
  ${(props) => !props.useModalDialog && "margin-bottom: 40px;"}
  ${(props) => !props.useModalDialog && !props.image && "max-width: none;"}

  .select_link {
    color: ${(props) => props.theme.avatarEditorBody.selectLink.color};
    a {
      color: ${(props) => props.theme.avatarEditorBody.selectLink.linkColor};
    }
  }
`;
StyledAvatarEditorBody.defaultProps = { theme: Base };

const StyledErrorContainer = styled.div`
  p {
    text-align: center;
  }
`;

const Slider = styled.input.attrs({
  type: "range",
})`
  width: ${(props) => props.theme.avatarEditorBody.slider.width};
  margin: ${(props) => props.theme.avatarEditorBody.slider.margin};
  background-color: ${(props) =>
    props.theme.avatarEditorBody.slider.backgroundColor};
  -webkit-appearance: none;

  &:focus {
    outline: none;
  }

  &::-webkit-slider-runnable-track {
    background: ${(props) =>
      props.theme.avatarEditorBody.slider.runnableTrack.background};
    border: ${(props) =>
      props.theme.avatarEditorBody.slider.runnableTrack.border};
    border-radius: ${(props) =>
      props.theme.avatarEditorBody.slider.runnableTrack.borderRadius};
    width: ${(props) =>
      props.theme.avatarEditorBody.slider.runnableTrack.width};
    height: ${(props) =>
      props.theme.avatarEditorBody.slider.runnableTrack.height};
    cursor: pointer;
  }

  &::-webkit-slider-thumb {
    margin-top: ${(props) =>
      props.theme.avatarEditorBody.slider.sliderThumb.marginTop};
    width: ${(props) => props.theme.avatarEditorBody.slider.sliderThumb.width};
    height: ${(props) =>
      props.theme.avatarEditorBody.slider.sliderThumb.height};
    background: ${(props) =>
      props.theme.avatarEditorBody.slider.sliderThumb.background};
    border: ${(props) =>
      props.theme.avatarEditorBody.slider.sliderThumb.border};
    border-radius: ${(props) =>
      props.theme.avatarEditorBody.slider.sliderThumb.height};
    cursor: pointer;
    -webkit-appearance: none;
    -webkit-box-shadow: ${(props) =>
      props.theme.avatarEditorBody.slider.sliderThumb.boxShadow};
    box-shadow: ${(props) =>
      props.theme.avatarEditorBody.slider.sliderThumb.boxShadow};
  }

  &:focus::-webkit-slider-runnable-track {
    background: ${(props) =>
      props.theme.avatarEditorBody.slider.runnableTrack.focusBackground};
  }

  &::-moz-range-track {
    background: ${(props) =>
      props.theme.avatarEditorBody.slider.rangeTrack.background};
    border: ${(props) =>
      props.theme.avatarEditorBody.slider.rangeTrack.background};
    border-radius: ${(props) =>
      props.theme.avatarEditorBody.slider.rangeTrack.borderRadius};
    width: ${(props) => props.theme.avatarEditorBody.slider.rangeTrack.width};
    height: ${(props) => props.theme.avatarEditorBody.slider.rangeTrack.height};
    cursor: pointer;
  }

  &::-moz-range-thumb {
    width: ${(props) => props.theme.avatarEditorBody.slider.rangeThumb.width};
    height: ${(props) => props.theme.avatarEditorBody.slider.rangeThumb.height};
    background: ${(props) =>
      props.theme.avatarEditorBody.slider.rangeThumb.background};
    border: ${(props) => props.theme.avatarEditorBody.slider.rangeThumb.border};
    border-radius: ${(props) =>
      props.theme.avatarEditorBody.slider.rangeThumb.borderRadius};
    cursor: pointer;
    -moz-box-shadow: ${(props) =>
      props.theme.avatarEditorBody.slider.rangeThumb.boxShadow};
    box-shadow: ${(props) =>
      props.theme.avatarEditorBody.slider.rangeThumb.boxShadow};
  }

  &::-ms-track {
    background: ${(props) =>
      props.theme.avatarEditorBody.slider.track.background};
    border-color: ${(props) =>
      props.theme.avatarEditorBody.slider.track.borderColor};
    border-width: ${(props) =>
      props.theme.avatarEditorBody.slider.track.borderWidth};
    color: ${(props) => props.theme.avatarEditorBody.slider.track.color};
    width: ${(props) => props.theme.avatarEditorBody.slider.track.width};
    height: ${(props) => props.theme.avatarEditorBody.slider.track.height};
    cursor: pointer;
  }

  &::-ms-fill-lower {
    background: ${(props) =>
      props.theme.avatarEditorBody.slider.fillLower.background};
    border: ${(props) => props.theme.avatarEditorBody.slider.fillLower.border};
    border-radius: ${(props) =>
      props.theme.avatarEditorBody.slider.fillLower.borderRadius};
  }

  &::-ms-fill-upper {
    background: ${(props) =>
      props.theme.avatarEditorBody.slider.fillUpper.background};
    border: ${(props) => props.theme.avatarEditorBody.slider.fillUpper.border};
    border-radius: ${(props) =>
      props.theme.avatarEditorBody.slider.fillUpper.borderRadius};
  }

  &::-ms-thumb {
    width: ${(props) => props.theme.avatarEditorBody.slider.thumb.width};
    height: ${(props) => props.theme.avatarEditorBody.slider.thumb.height};
    background: ${(props) =>
      props.theme.avatarEditorBody.slider.thumb.background};
    border: ${(props) => props.theme.avatarEditorBody.slider.thumb.border};
    border-radius: ${(props) =>
      props.theme.avatarEditorBody.slider.thumb.borderRadius};
    cursor: pointer;
    margin-top: ${(props) =>
      props.theme.avatarEditorBody.slider.thumb.marginTop};
    /*Needed to keep the Edge thumb centred*/
    box-shadow: ${(props) =>
      props.theme.avatarEditorBody.slider.thumb.boxShadow};
  }

  &:focus::-ms-fill-lower {
    background: ${(props) =>
      props.theme.avatarEditorBody.slider.fillLower.focusBackground};
  }

  &:focus::-ms-fill-upper {
    background: ${(props) =>
      props.theme.avatarEditorBody.slider.fillUpper.focusBackground};
  }
`;
Slider.defaultProps = { theme: Base };

const DropZoneContainer = styled.div`
  outline: none;

  .dropzone-text {
    border: ${(props) => props.theme.avatarEditorBody.dropZone.border};
    display: flex;
    align-items: center;
    justify-content: center;

    box-sizing: border-box;
    width: 100%;
    height: 100%;

    cursor: pointer;

    @media ${desktop} {
      width: 408px;
      height: 360px;
    }

    @media ${tablet} {
      height: 426px;
    }
  }
`;
DropZoneContainer.defaultProps = { theme: Base };

const mobileStyles = css`
  grid-template-rows: 1fr auto;

  .preview-container {
    margin-bottom: 23px;

    .editor-container {
      display: grid;
      grid-template-columns: 1fr 40px;
      grid-template-rows: 1fr auto;
      gap: 0px 16px;

      @media ${mobile} {
      }

      .react-avatar-editor {
      }

      .editor-buttons {
        grid-template-columns: 1fr;
        width: ${(props) =>
          props.theme.avatarEditorBody.container.buttons.mobileWidth};
        grid-template-rows: repeat(4, 40px) auto 40px;
        height: ${(props) =>
          props.theme.avatarEditorBody.container.buttons.mobileHeight};
        grid-gap: 8px 0;
        background: ${(props) =>
          props.theme.avatarEditorBody.container.buttons.mobileBackground};

        .editor-button {
          background: ${(props) =>
            props.theme.avatarEditorBody.container.button.background};
          padding: ${(props) =>
            props.theme.avatarEditorBody.container.button.padding};
          height: ${(props) =>
            props.theme.avatarEditorBody.container.button.height};
          border-radius: ${(props) =>
            props.theme.avatarEditorBody.container.button.borderRadius};

          display: flex;
          align-items: center;
        }
      }

      .zoom-container {
        height: ${(props) =>
          props.theme.avatarEditorBody.container.zoom.mobileHeight};
        margin-top: ${(props) =>
          props.theme.avatarEditorBody.container.zoom.marginTop};
        .zoom-container-svg_zoom-minus,
        .zoom-container-svg_zoom-plus {
          margin: auto 0;
        }
      }
    }
  }
`;

const StyledAvatarContainer = styled.div`
  display: grid;
  grid-template-columns: 1fr;
  grid-template-rows: 1fr 50px;

  width: 100%;
  @media ${desktop} {
    width: 408px;
  }

  .preview-container {
    display: grid;
    grid-template-columns: 1fr;
    grid-column-gap: 16px;

    .custom-range {
      width: 100%;
      display: block;
    }

    @media ${desktop} {
      grid-template-columns: min-content 1fr;
      .avatar-container {
        width: 160px;
        display: grid;
        grid-template-rows: 160px 48px;
        grid-row-gap: 16px;

        .avatar-mini-preview {
          width: ${(props) =>
            props.theme.avatarEditorBody.container.miniPreview.width};

          border: ${(props) =>
            props.theme.avatarEditorBody.container.miniPreview.border};
          box-sizing: border-box;
          border-radius: ${(props) =>
            props.theme.avatarEditorBody.container.miniPreview.borderRadius};
          display: grid;
          grid-template-columns: 38px 88px 1fr;
          align-items: center;
          padding: ${(props) =>
            props.theme.avatarEditorBody.container.miniPreview.padding};
        }
      }
    }

    .editor-container {
      width: 100%;
      display: grid;

      @media ${desktop} {
        width: 224px;
      }

      @media ${tablet} {
        canvas {
          width: 100% !important;
          height: initial !important;
        }
      }

      .editor-buttons {
        display: grid;
        grid-template-columns: repeat(4, 1fr) 2fr 1fr;
        grid-template-rows: 32px;
        height: ${(props) =>
          props.theme.avatarEditorBody.container.buttons.height};
        background: ${(props) =>
          props.theme.avatarEditorBody.container.buttons.background};
        justify-items: center;
        .editor-button {
          margin: auto 0;
          svg {
            path {
              fill: ${(props) =>
                props.theme.avatarEditorBody.container.button.fill};
            }
          }

          &:hover {
            svg {
              path {
                fill: ${(props) =>
                  props.theme.avatarEditorBody.container.button.hoverFill};
              }
            }
          }
        }
      }

      .zoom-container {
        height: ${(props) =>
          props.theme.avatarEditorBody.container.zoom.height};
        display: grid;
        grid-template-columns: min-content 1fr min-content;
        grid-column-gap: 12px;
        .zoom-container-svg_zoom-minus,
        .zoom-container-svg_zoom-plus {
          margin: auto 0;
        }
      }
    }
  }

  ${(props) => !props.useModalDialog && mobileStyles}
`;
StyledAvatarContainer.defaultProps = { theme: Base };

export {
  StyledAvatarEditorBody,
  StyledAvatarContainer,
  DropZoneContainer,
  Slider,
  StyledErrorContainer,
};

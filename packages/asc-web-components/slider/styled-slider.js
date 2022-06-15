import styled from "styled-components";
import { Base } from "../themes";

const StyledSlider = styled.input.attrs({
  type: "range",
})`
  width: ${(props) => props.theme.avatarEditorBody.slider.width};
  margin: ${(props) => props.theme.avatarEditorBody.slider.margin};
  background: ${(props) =>
    props.theme.avatarEditorBody.slider.runnableTrack.focusBackground};

  border-radius: ${(props) =>
    props.theme.avatarEditorBody.slider.runnableTrack.borderRadius};

  -webkit-appearance: none;

  background-image: ${(props) =>
    props.withPouring && `linear-gradient(#2da7db, #2da7db)`};
  background-size: ${(props) => `${props.size} 100%`};
  background-repeat: no-repeat;

  &:focus {
    outline: none;
  }

  &::-webkit-slider-runnable-track {
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
StyledSlider.defaultProps = { theme: Base };

const StyledRangeSlider = styled.div`
  background: red;
`;

export { StyledSlider, StyledRangeSlider };

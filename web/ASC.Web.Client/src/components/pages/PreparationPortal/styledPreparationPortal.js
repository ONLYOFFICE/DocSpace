import styled from "styled-components";

const StyledPreparationPortal = styled.div`
  margin-bottom: 24px;
  display: flex;
  width: 100%;
  max-width: ${(props) => (props.errorMessage ? "560px" : "360px")};
  padding: 0 24px;
  box-sizing: border-box;
  align-items: center;

  .preparation-portal_progress-bar {
    border-radius: 2px;
    margin-right: 8px;
    width: 100%;

    height: 14px;
    background-color: #a3a9ae;
  }
  .preparation-portal_progress-line {
    border-radius: inherit;
    width: ${(props) => props.percent}%;
    background: #439ccd;
    height: inherit;
    //width: 0;
    transition-property: width;
    transition-duration: 0.9s;
    background: linear-gradient(
      top,
      #57aedf,
      #439ccd 50%,
      #3b95c6 51%,
      #237faf
    );
    background: -o-linear-gradient(
      top,
      #57aedf,
      #439ccd 50%,
      #3b95c6 51%,
      #237faf
    );
    background: -moz-linear-gradient(
      top,
      #57aedf,
      #439ccd 50%,
      #3b95c6 51%,
      #237faf
    );
    background: -webkit-linear-gradient(
      top,
      #57aedf,
      #439ccd 50%,
      #3b95c6 51%,
      #237faf
    );
    -ms-filter: "progid:DXImageTransform.Microsoft.gradient(startColorStr='#57aedf', EndColorStr='#237faf')";
  }
`;
export default StyledPreparationPortal;

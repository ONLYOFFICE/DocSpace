import styled from "styled-components";

const StyledSettings = styled.div`
  margin-top: ${(props) => (props.showTitle ? 24 : 34)}px;
  width: 100%;

  display: grid;
  grid-gap: 32px;

  .toggle-btn {
    position: relative;
  }

  .heading {
    margin-bottom: -2px;
    margin-top: 0;
  }

  .toggle-button-text {
    margin-top: -1px;
  }

  .settings-section {
    display: grid;
    grid-gap: 18px;
  }
`;

export default StyledSettings;

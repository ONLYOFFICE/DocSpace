import styled, { css } from "styled-components";

const StyledAlertComponent = styled.div`
  position: relative;
  border: ${(props) => `1px solid ${props.borderColor}`};
  border-radius: 6px;
  padding: 12px;
  ${(props) => !!props.onClick && "cursor:pointer"};
  display: grid;

  grid-template-columns: ${(props) =>
    props.needArrowIcon ? "1fr 16px" : "1fr"};

  .alert-component_title {
    color: ${(props) => props.titleColor};
  }
  .alert-component_icons {
    margin: auto 0;
  }
`;

export { StyledAlertComponent };

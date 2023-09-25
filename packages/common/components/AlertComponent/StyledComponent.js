import styled, { css } from "styled-components";

const StyledAlertComponent = styled.div`
  width: 100%;
  position: relative;
  border: ${(props) => `1px solid ${props.borderColor}`};
  border-radius: 6px;
  padding: 12px;
  ${(props) => !!props.onClick && "cursor:pointer"};
  display: grid;

  grid-template-columns: ${(props) =>
    props.needArrowIcon ? "1fr 16px" : "1fr"};

  .main-content {
    display: flex;
    flex-direction: column;
    align-items: start;
    justify-content: center;
    gap: 4px;
  }

  .alert-component_title {
    color: ${(props) => props.titleColor};
  }
  .alert-component_icons {
    margin: auto 0;
  }
`;

export { StyledAlertComponent };

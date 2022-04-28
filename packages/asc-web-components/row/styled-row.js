import styled from "styled-components";

import { tablet } from "../utils/device";
import Base from "../themes/base";

const StyledRow = styled.div`
  cursor: default;
  position: relative;
  min-height: ${(props) => props.theme.row.minHeight};
  width: ${(props) => props.theme.row.width};
  border-bottom: 2px solid transparent;

  ::after {
    position: absolute;
    display: block;
    bottom: 0px;
    width: 100%;
    height: 1px;
    background-color: ${(props) => props.theme.row.borderBottom};
    content: "";
  }

  display: flex;
  flex-direction: row;
  flex-wrap: nowrap;

  justify-content: flex-start;
  align-items: center;
  align-content: center;

  .row-loader {
    padding: 15px 12px 12px 0px;
  }
`;
StyledRow.defaultProps = { theme: Base };

const StyledContent = styled.div`
  display: flex;
  flex-basis: 100%;

  min-width: ${(props) => props.theme.row.minWidth};

  @media ${tablet} {
    white-space: nowrap;
    overflow: ${(props) => props.theme.row.overflow};
    text-overflow: ${(props) => props.theme.row.textOverflow};
    height: ${(props) => props.theme.rowContent.height};
  }
`;
StyledContent.defaultProps = { theme: Base };

const StyledCheckbox = styled.div`
  flex: 0 0 16px;

  .checkbox {
    padding: 7px 0;

    @media ${tablet} {
      padding: 10px 0;
    }
  }
`;

const StyledElement = styled.div`
  flex: 0 0 auto;
  display: flex;
  margin-right: ${(props) => props.theme.row.element.marginRight};
  margin-left: ${(props) => props.theme.row.element.marginLeft};
  user-select: none;

  .react-svg-icon svg {
    margin-top: 4px;
  }
  /* .react-svg-icon.is-edit svg {
    margin: 4px 0 0 28px;
  } */
`;
StyledElement.defaultProps = { theme: Base };

const StyledContentElement = styled.div`
  margin-top: 6px;
  user-select: none;
`;

const StyledOptionButton = styled.div`
  display: flex;
  width: ${(props) => props.spacerWidth && props.spacerWidth};
  justify-content: flex-end;

  .expandButton > div:first-child {
    padding: ${(props) => props.theme.row.optionButton.padding};

    margin-right: 0px;

    @media (min-width: 1024px) {
      margin-right: -1px;
    }
    @media (max-width: 516px) {
      padding-left: 10px;
    }
  }

  //margin-top: -1px;
  @media ${tablet} {
    margin-top: unset;
  }
`;
StyledOptionButton.defaultProps = { theme: Base };

export {
  StyledOptionButton,
  StyledContentElement,
  StyledElement,
  StyledCheckbox,
  StyledContent,
  StyledRow,
};

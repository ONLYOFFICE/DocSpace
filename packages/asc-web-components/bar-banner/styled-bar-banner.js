import styled from "styled-components";
import CrossIcon from "../../../public/images/cross.react.svg";
import commonIconsStyles from "../utils/common-icons-style";

const StyledCrossIcon = styled(CrossIcon)`
  ${commonIconsStyles}

  &:hover {
    cursor: pointer;
  }

  path {
    fill: #fff;
  }
`;

const StyledBarBanner = styled.div`
  width: 100%;
  height: 100%;
  display: ${(props) => (props.open ? "flex" : "none")};
  justify-content: space-between;
  align-items: flex-start;
  position: relative;

  .action {
    padding: 4px;
    position: absolute;
    left: 98%;
  }
`;

export { StyledBarBanner, StyledCrossIcon };

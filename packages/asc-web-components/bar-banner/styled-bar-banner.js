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
  background: #e6eb96;
  width: 100%;
  height: 100%;
  display: ${(props) => (props.open ? "flex" : "none")};
  justify-content: space-between;
  align-items: flex-start;

  .action {
    padding: 4px;
  }
`;

export { StyledBarBanner, StyledCrossIcon };

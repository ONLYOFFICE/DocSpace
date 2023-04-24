import styled from "styled-components";

type StyledButtonScrollProps = {
  orientation: "left" | "right";
};

const StyledButtonScroll = styled.div<StyledButtonScrollProps>`
  z-index: 307;
  position: fixed;
  top: calc(50% - 20px);

  ${(props) => (props.orientation === "left" ? "left: 20px;" : "right: 20px;")}
`;

export default StyledButtonScroll;

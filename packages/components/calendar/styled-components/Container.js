import styled from "styled-components";

export const Container = styled.div`
  box-sizing: border-box;

  width: ${(props) => (props.isMobile ? "100%" : "432px")};
  height: ${(props) => (props.isMobile ? "420px" : "446px")};
  padding: ${(props) => (props.isMobile ? "16px" : "30px 28px 28px 28px")};
  box-shadow: 0px 12px 40px rgba(4, 15, 27, 0.12);
  border-radius: 6px;
  z-index: 320;
  background-color: ${(props) => props.theme.backgroundColor};
`;

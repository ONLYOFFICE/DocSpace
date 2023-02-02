import styled from "styled-components";

export const Title = styled.h2`
  font-family: "Open Sans", sans-serif, Arial;
  font-weight: 700;
  font-size: 21px;
  line-height: 28px;
  color: #555f65;
  border-bottom: 1px dashed transparent;
  margin: 0;
  display: inline-block;

  :hover {
    border-bottom: ${(props) =>
      props.disabled ? "none" : "1px dashed #555f65"};
    cursor: ${(props) => (props.disabled ? "auto" : "pointer")};
  }
`;

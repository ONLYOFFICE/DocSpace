import styled from "styled-components";
import RectangleLoader from "../RectangleLoader";
import { tablet, mobile } from "@appserver/components/utils/device";

const StyledContainer = styled.div`
  margin: 0;

  max-width: 216px;
  padding: 0 20px;

  display: flex;
  flex-direction: column;

  @media ${tablet} {
    width: ${(props) => (props.showText ? "240px" : "52px")};
    padding: 0 16px;
  }

  @media ${mobile} {
    width: 100%;
  }
`;

const StyledRectangleLoader = styled(RectangleLoader)`
  height: 20px;
  width: 216px;
  padding: 0 0 16px;

  @media ${tablet} {
    height: 20px;
    width: 20px;
    padding: 0 0 24px;
  }
`;

export { StyledContainer, StyledRectangleLoader };

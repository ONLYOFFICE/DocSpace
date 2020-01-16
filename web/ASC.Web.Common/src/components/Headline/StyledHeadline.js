import styled from "styled-components";
import { Heading } from "asc-web-components";

const size = {
  header: "28px",
  menu: "27px",
  content: "21px"
};

const weight = {
  header: 600,
  menu: "bold",
  content: "bold"
};

const StyledHeading = styled(Heading)`
  margin: 0;
  line-height: 56px;
  font-size: ${props => size[props.headlineType]};
  font-weight: ${props => weight[props.headlineType]};
`;

export default StyledHeading;

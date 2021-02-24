import styled from "styled-components";
import Heading from "@appserver/components/src/components/heading";
import { Base } from "@appserver/components/src/themes";

const size = {
  header: "28px",
  menu: "27px",
  content: "21px",
};

const weight = {
  header: 600,
  menu: "bold",
  content: "bold",
};

const StyledHeading = styled(Heading)`
  margin: 0;
  line-height: 65px;
  font-size: ${(props) => size[props.headlineType]};
  font-weight: ${(props) => weight[props.headlineType]};
  color: ${(props) => props.theme.color};
`;
StyledHeading.defaultProps = { theme: Base };

export default StyledHeading;

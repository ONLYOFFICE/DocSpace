import styled from "styled-components";
import Heading from "@appserver/components/heading";
import Base from "@appserver/components/themes/base";
import NoUserSelect from "@appserver/components/utils/commonStyles";
const size = {
  header: "28px",
  menu: "23px",
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
  color: ${(props) => (props.color ? props.color : props.theme.color)};
  ${NoUserSelect}
`;
StyledHeading.defaultProps = { theme: Base };

export default StyledHeading;

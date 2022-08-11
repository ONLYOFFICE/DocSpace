import styled from "styled-components";
import Heading from "@docspace/components/heading";
import Base from "@docspace/components/themes/base";
import NoUserSelect from "@docspace/components/utils/commonStyles";
import { tablet } from "@docspace/components/utils/device";
const size = {
  header: "28px",
  menu: "23px",
  content: "18px",
};

const weight = {
  header: 600,
  menu: "bold",
  content: "bold",
};

const StyledHeading = styled(Heading)`
  margin: 0;
  line-height: 50px;
  font-size: ${(props) =>
    props.fontSize ? props.fontSize : size[props.headlineType]};
  font-weight: ${(props) => weight[props.headlineType]};
  color: ${(props) => (props.color ? props.color : props.theme.color)};
  ${NoUserSelect}
  @media ${tablet} {
    ${(props) => props.headlineType === "content" && "font-size: 18px"};
  }
`;
StyledHeading.defaultProps = { theme: Base };

export default StyledHeading;

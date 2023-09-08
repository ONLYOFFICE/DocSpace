import styled from "styled-components";
import { Base } from "@docspace/components/themes";

const StyledSettingsSeparator = styled.hr`
  margin: 24px 0;
  border: none;
  border-top: ${(props) => props.theme.client.settings.separatorBorder};
  max-width: 700px;
`;

StyledSettingsSeparator.defaultProps = { theme: Base };

export default StyledSettingsSeparator;

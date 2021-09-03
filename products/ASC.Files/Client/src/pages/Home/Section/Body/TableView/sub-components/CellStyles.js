import styled from "styled-components";
import Text from "@appserver/components/text";

const StyledText = styled(Text)`
  display: inline-block;
  margin-right: 12px;
`;

const StyledAuthorAvatar = styled.img`
  width: 16px;
  height: 16px;
  margin-right: 8px;
  border-radius: 20px;
`;

export { StyledText, StyledAuthorAvatar };

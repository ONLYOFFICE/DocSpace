import Box from "@docspace/components/box";

import IdpSettings from "./IdpSettings";
import Certificates from "./Certificates";
import FieldMapping from "./FieldMapping";
import SubmitResetButtons from "./SubmitButton";

const SPSettings = () => {
  return (
    <Box className="service-provider-settings">
      <IdpSettings />
      <Certificates provider="IdentityProvider" />
      <Certificates provider="ServiceProvider" />
      <FieldMapping />
      <SubmitResetButtons />
    </Box>
  );
};

export default SPSettings;

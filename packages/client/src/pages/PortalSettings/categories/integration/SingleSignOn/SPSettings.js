import { useEffect } from "react";
import { useNavigate, useLocation } from "react-router-dom";

import Box from "@docspace/components/box";
import { size } from "@docspace/components/utils/device";

import IdpSettings from "./IdpSettings";
import Certificates from "./Certificates";
import FieldMapping from "./FieldMapping";
import SubmitResetButtons from "./SubmitButton";

const SPSettings = () => {
  const navigate = useNavigate();
  const location = useLocation();

  useEffect(() => {
    checkWidth();
    window.addEventListener("resize", checkWidth);
    return () => window.removeEventListener("resize", checkWidth);
  }, []);

  const checkWidth = () => {
    window.innerWidth > size.smallTablet &&
      location.pathname.includes("sp-settings") &&
      navigate("/portal-settings/integration/single-sign-on");
  };

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

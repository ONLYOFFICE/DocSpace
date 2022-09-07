import PhoneInput from "react-phone-input-2";
import "react-phone-input-2/lib/style.css";

export const InputPhone = () => {
  return (
    <PhoneInput
      inputExtraProps={{
        name: "phone",
        required: true,
        autoFocus: true,
      }}
      placeholder="XXX XXX-XX-XX"
      defaultCountry="ru"
    />
  );
};

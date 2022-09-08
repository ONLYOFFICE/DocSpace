import { useState } from "react";
import ReactPhoneInput from "react-phone-input-2";
import styled from "styled-components";
import "react-phone-input-2/lib/style.css";

const PhoneInput = styled(ReactPhoneInput)`
  border-radius: 3px;
  &:focus-within {
    width: 330px;
    border: 1px solid #2da7db;
  }
`;
export const InputPhone = () => {
  const [phoneValue, setPhoneValue] = useState("");

  return (
    <PhoneInput
      inputProps={{
        name: "phone",
        required: true,
        autoFocus: true,
      }}
      value={phoneValue}
      onChange={(phone) => setPhoneValue(phone)}
      country={"ru"}
      buttonStyle={{ background: "#fff", width: "50px" }}
      inputStyle={{
        width: "320px",
        height: "44px",
        marginLeft: "10px",
        borderRadius: "3px",
        fontSize: "16px",
      }}
      searchPlaceholder="Search"
      containerStyle={{ marginTop: "15px" }}
      searchStyle={{
        margin: "0",
        width: "97%",
        height: "30px",
        borderColor: "#2da7db",
      }}
      enableSearch
      disableSearchIcon
      placeholder="XXX XXX-XX-XX"
      defaultErrorMessage="Сountry code is invalid"
    />
  );
};

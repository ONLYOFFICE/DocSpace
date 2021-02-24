import React, { useEffect, useState } from "react";
//import Routes from "./Routes";
//import { Box, Text } from "@appserver/components";
import Box from "@appserver/components/box";
import Text from "@appserver/components/text";

const PeopleContent = (props) => {
  const [isLoading, setIsLoading] = useState(true);
  console.log("People props", props);

  useEffect(() => {
    setIsLoading(false);
  }, []);

  return (
    <Box
      displayProp="flex"
      flexDirection="column"
      alignItems="center"
      widthProp="100%"
    >
      <Box displayProp="flex" alignItems="center" heightProp="100%">
        <Text fontSize="24px" color="green">
          PEOPLE PAGE
        </Text>
      </Box>
    </Box>
  ); //isLoading ? <div>LOADING STORE</div> : <Routes />;
};

export default PeopleContent;

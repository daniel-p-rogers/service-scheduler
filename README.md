# To publish

Navigate to `.\ServiceScheduler` and run the below:

`dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true`

Then navigate to `.\ServiceScheduler\ServiceScheduler\bin\Release\net6.0\win-x64\publish` and copy the content to your published folder.

# To run

Ensure all the files below are up to date:
- People.csv
- ServiceDates.csv
- Unavailability.csv

Once these are populated, run `ServiceScheduler.exe`. The proposed schedule will be added to the `OutputRotas` folder.
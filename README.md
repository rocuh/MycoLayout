# Myco Layout
**Myco Layout** is a Xamarin.Forms control for laying out simple SkiaSharp based controls with XAML, primarly to improve performance on Android.

## Supported Platforms
The library currently supports native renderers for the following platforms:

- Android
- iOS

## Example
In your Xamarin.Forms project, add a new MycoContainer element to your page, and add the MycoGrid, MycoLabel, MycoImage, MycoImageButton controls.

```xml
  <ml:MycoContainer>
	<ml:MycoGrid Padding="5,5,5,5">              
	  <ml:MycoGrid.RowDefinitions>
		<RowDefinition Height="Auto"/>
		<RowDefinition Height="Auto"/>
	  </ml:MycoGrid.RowDefinitions>

	  <ml:MycoGrid.ColumnDefinitions>
		<ColumnDefinition Width="50" />
		<ColumnDefinition Width="*" />
		<ColumnDefinition Width="*" />
	  </ml:MycoGrid.ColumnDefinitions>

	  <ml:MycoLabel ml:MycoGrid.Row="0" ml:MycoGrid.Column="0" ml:MycoGrid.ColumnSpan="3" TextColor="#333333" Text="{Binding Text}" FontSize="20" FontAttributes="Bold" VerticalTextAlignment="Center" />

	  <ml:MycoLabel ml:MycoGrid.Row="1" ml:MycoGrid.Column="1" TextColor="#333333" FontSize="12" Text="{Binding Labels[0].Header}" IsVisible="{Binding Labels[0].Enabled}"/>
	  <ml:MycoLabel ml:MycoGrid.Row="1" ml:MycoGrid.Column="2" TextColor="#333333" FontSize="12" Text="{Binding Labels[0].Value}" HorizontalTextAlignment="End" IsVisible="{Binding Labels[0].Enabled}"/>

	</ml:MycoGrid>
  </ml:MycoContainer>     
```

For iOS you need to add MycoContainerRenderer.Initialize() into you AppDelegate
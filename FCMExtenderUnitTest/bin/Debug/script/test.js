function totVoti(arr){
	var sum=0;
	for (var i=0; i<arr.length; i++){
		console.log(arr[i].voto);
		sum += arr[i].voto;
	}
	return sum;
}

function mediaVoti(arr){
	if (arr.length>0){
		return totVoti(arr)/arr.length;
	}
	else {
		return 0;
	}
}

function modificatoreDifesaFCM(formazione){
	var titolari = formazione.slice(0,11);
	var difensori = titolari.filter(function (item){
		return item.ruolo==2 || item.ruolo==6;
	});
	console.log ("difensori.len: "+difensori.length);
	//aggiungo i 5 d'ufficio
	for (var i=0; i<difensori.length; i++){
		if (difensori[i].voto==0){
			difensori[i].voto = 5;
		}
	}
	var mvDif = mediaVoti(difensori);
	mvDif = Math.max(4.99,Math.min(7,mvDif)); //restringo il valore tra 4.99 e 7. Oltre questi intervalli il valore non cambia
	console.log ("mv: "+(mvDif));
	console.log ("dif: "+(5.75-mvDif));
	console.log ("modA: "+Math.ceil((5.75-mvDif)*4));
	console.log ("modB: "+(4-difensori.length));
	console.log ("modTot: "+(Math.ceil((5.75-mvDif)*4)+(4-difensori.length)));
	
	return Math.ceil((5.75-mvDif)*4)+(4-difensori.length);
}

incontro.casa.ModM1Pers = modificatoreDifesaFCM(incontro.trasferta.Formazione);
incontro.trasferta.ModM1Pers = modificatoreDifesaFCM(incontro.casa.Formazione);